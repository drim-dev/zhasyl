import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Aspire exposes the local dev server through this loopback origin.
  allowedDevOrigins: ["127.0.0.1"],
};

export default nextConfig;
