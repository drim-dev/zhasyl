import NextAuth from "next-auth";
import type { NextAuthConfig } from "next-auth";
import Credentials from "next-auth/providers/credentials";
import GitHub from "next-auth/providers/github";
import GitLab from "next-auth/providers/gitlab";
import Google from "next-auth/providers/google";
import { z } from "zod";

const developmentCredentials = z.object({ email: z.string().email() });
const adultIdentity = z.object({
  adultId: z.string().uuid(),
  email: z.string().email(),
});

async function resolveAdult(
  provider: string,
  providerUserId: string,
  providerEmail: string,
): Promise<z.infer<typeof adultIdentity> | null> {
  const apiBaseUrl = process.env.API_BASE_URL;
  if (!apiBaseUrl) return null;

  for (let attempt = 0; attempt < 3; attempt += 1) {
    try {
      const response = await fetch(
        new URL("/api/auth/oauth-sign-in", apiBaseUrl),
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ provider, providerUserId, providerEmail }),
        },
      );
      if (response.ok) {
        const parsed = adultIdentity.safeParse(await response.json());
        return parsed.success ? parsed.data : null;
      }
      if (response.status < 500) return null;
    } catch {
      // The private API may still be settling during local orchestration startup.
    }
    await new Promise((resolve) => setTimeout(resolve, 250 * (attempt + 1)));
  }
  return null;
}

const providers: NextAuthConfig["providers"] = [];
const googleConfigured = Boolean(
  process.env.AUTH_GOOGLE_ID && process.env.AUTH_GOOGLE_SECRET,
);
const githubConfigured = Boolean(
  process.env.AUTH_GITHUB_ID && process.env.AUTH_GITHUB_SECRET,
);
const gitlabConfigured = Boolean(
  process.env.AUTH_GITLAB_ID && process.env.AUTH_GITLAB_SECRET,
);

if (googleConfigured) {
  providers.push(
    Google({
      clientId: process.env.AUTH_GOOGLE_ID,
      clientSecret: process.env.AUTH_GOOGLE_SECRET,
    }),
  );
}

if (githubConfigured) {
  providers.push(
    GitHub({
      clientId: process.env.AUTH_GITHUB_ID,
      clientSecret: process.env.AUTH_GITHUB_SECRET,
    }),
  );
}

if (gitlabConfigured) {
  providers.push(
    GitLab({
      clientId: process.env.AUTH_GITLAB_ID,
      clientSecret: process.env.AUTH_GITLAB_SECRET,
    }),
  );
}

if (process.env.NODE_ENV !== "production") {
  providers.push(
    Credentials({
      id: "development",
      name: "Локальная разработка",
      credentials: {
        email: { label: "Email", type: "email" },
      },
      async authorize(credentials) {
        const result = developmentCredentials.safeParse(credentials);
        if (!result.success) {
          return null;
        }
        const adult = await resolveAdult(
          "development",
          result.data.email,
          result.data.email,
        );
        if (!adult) return null;

        return {
          id: adult.adultId,
          email: adult.email,
          name: "Локальный взрослый",
        };
      },
    }),
  );
}

export const configuredSocialProviders = [
  googleConfigured ? "google" : null,
  githubConfigured ? "github" : null,
  gitlabConfigured ? "gitlab" : null,
].filter((provider): provider is string => provider !== null);

export const { handlers, auth, signIn, signOut } = NextAuth({
  trustHost: true,
  providers,
  session: { strategy: "jwt" },
  callbacks: {
    async signIn({ user, account }) {
      if (!account || !user.email) {
        return false;
      }
      if (account.provider === "development") return true;

      const adult = await resolveAdult(
        account.provider,
        account.providerAccountId,
        user.email,
      );
      if (!adult) return false;

      user.id = adult.adultId;
      return true;
    },
    jwt({ token, user }) {
      if (user?.id) {
        token.adultId = user.id;
      }
      return token;
    },
    session({ session, token }) {
      if (session.user && typeof token.adultId === "string") {
        session.user.id = token.adultId;
      }
      return session;
    },
  },
  pages: {
    signIn: "/adult/sign-in",
    error: "/adult/sign-in",
  },
});
