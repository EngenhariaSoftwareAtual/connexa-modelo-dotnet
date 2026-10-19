#!/usr/bin/env bash
# Executado uma vez, quando o Codespace é criado: baixa as dependências
# para que o primeiro ./verificar não precise esperar downloads.
set -euo pipefail

echo "==> Back-end: restaurando pacotes do NuGet e ferramentas (dotnet-ef)"
(cd backend && dotnet restore Connexa.slnx && dotnet tool restore) || echo "Aviso: não foi possível pré-baixar os pacotes do NuGet."

echo "==> Front-end: instalando dependências do npm"
(cd frontend && npm ci)

echo ""
echo "Ambiente pronto. Próximos passos:"
echo "  ./verificar                                            roda tudo o que o CI roda"
echo "  cd backend && dotnet run --project src/Connexa.Api     sobe a API em http://localhost:8080"
echo "  cd frontend && npm run dev                             sobe a interface em http://localhost:5173"
