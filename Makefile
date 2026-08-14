.PHONY: restore dev build test test-e2e format format-check verify

restore:
	dotnet restore Zhasyl.sln
	npm ci --prefix frontend

dev:
	dotnet run --project Zhasyl.AppHost/Zhasyl.AppHost.csproj

build:
	dotnet build Zhasyl.sln
	npm run build --prefix frontend

test:
	dotnet test Zhasyl.sln
	npm test --prefix frontend -- --runInBand

test-e2e:
	npm run test:e2e --prefix frontend

format:
	dotnet format Zhasyl.sln
	npm run format --prefix frontend

format-check:
	dotnet format Zhasyl.sln --verify-no-changes
	npm run format:check --prefix frontend

verify:
	dotnet build Zhasyl.sln
	dotnet test Zhasyl.sln --no-build
	npm run lint --prefix frontend
	npm run typecheck --prefix frontend
	npm test --prefix frontend -- --runInBand
	npm run build --prefix frontend
	npm audit --prefix frontend
