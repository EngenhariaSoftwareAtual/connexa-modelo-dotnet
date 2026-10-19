# Roda localmente tudo o que o CI roda (Windows / PowerShell). Use antes de todo push.
#   .\verificar.ps1            back-end e front-end
#   .\verificar.ps1 backend    só o back-end
#   .\verificar.ps1 frontend   só o front-end
#
# O back-end precisa do PostgreSQL em localhost:5432: docker compose up -d db
param([ValidateSet('tudo', 'backend', 'frontend')][string]$Alvo = 'tudo')
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

function Executar([string]$Pasta, [scriptblock]$Comando) {
  Push-Location $Pasta
  try {
    & $Comando
    if ($LASTEXITCODE -ne 0) { throw "Falha em '$Pasta' (código $LASTEXITCODE)." }
  } finally { Pop-Location }
}

function Verificar-Backend {
  Write-Host '==> Back-end: compilação, testes de aceitação e fronteiras entre módulos'
  Executar 'backend' {
    dotnet build Connexa.slnx
    if ($LASTEXITCODE -ne 0) { return }
    dotnet test --no-build
  }
}

function Verificar-Frontend {
  Write-Host '==> Front-end: lint, testes e build'
  Executar 'frontend' {
    if (-not (Test-Path 'node_modules')) { npm ci; if ($LASTEXITCODE -ne 0) { return } }
    npm run verificar
  }
}

function Verificar-Instrucoes {
  Write-Host '==> Instruções do agente sincronizadas'
  $a = Get-FileHash 'AGENTS.md'
  $b = Get-FileHash '.github/copilot-instructions.md'
  if ($a.Hash -ne $b.Hash) { throw 'AGENTS.md e .github/copilot-instructions.md estão diferentes.' }
}

switch ($Alvo) {
  'backend'  { Verificar-Backend }
  'frontend' { Verificar-Frontend }
  default    { Verificar-Instrucoes; Verificar-Backend; Verificar-Frontend }
}

Write-Host ''
Write-Host 'Verificação concluída sem falhas.'
