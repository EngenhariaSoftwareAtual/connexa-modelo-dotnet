# Migrações do banco (EF Core)

O esquema do banco muda **somente** por migrações. Nunca altere tabelas à mão nem use `EnsureCreated`.

## Primeira tabela de um módulo

1. Crie o contexto em `src/Connexa.<Modulo>/Internal/`, como classe `internal sealed`:
   ```csharp
   internal sealed class GruposContexto(DbContextOptions<GruposContexto> opcoes) : DbContext(opcoes)
   {
       public DbSet<Grupo> Grupos => Set<Grupo>();

       protected override void OnModelCreating(ModelBuilder modelo)
       {
           modelo.HasDefaultSchema(ModuloGrupos.Schema);
           modelo.Entity<Grupo>(grupo =>
           {
               grupo.ToTable("grupo");
               grupo.Property(g => g.Id).HasColumnName("id");
               grupo.Property(g => g.Nome).HasColumnName("nome");
           });
       }
   }
   ```
2. Registre-o em `Modulo<Modulo>.cs`: `servicos.AdicionarContextoDoModulo<GruposContexto>(Schema);`

## Criar uma migração

Na pasta `backend/`:

```bash
dotnet tool restore
dotnet ef migrations add <OQueFaz> \
  --project src/Connexa.Grupos --startup-project src/Connexa.Api \
  --context GruposContexto --output-dir Internal/Migracoes
```

- Exemplo de nome: `CriaTabelaGrupo`. O EF Core acrescenta a data e a hora no início do arquivo, o que evita que duas pessoas criem migrações com o mesmo nome em branches diferentes.
- As migrações são aplicadas quando a aplicação sobe. O histórico fica na tabela `__historico_de_migracoes` do schema do módulo.
- As classes geradas pelo EF Core em `Internal/Migracoes` são públicas; o `ArquiteturaModularTest` aceita essa exceção.

## O que não fazer

- Não edite uma migração que já foi integrada no `main`. Para mudar algo, crie uma nova migração.
- Não crie chave estrangeira para tabela de outro módulo. Guarde apenas o identificador (por exemplo, `estudante_id uuid`).
- Não leia nem escreva tabelas de outro módulo, nem use o contexto de outro módulo.

Os schemas de todos os módulos já existem: cada `Modulo<Modulo>.cs` registra o seu com `RegistrarSchemaDoModulo`, e eles são criados quando a aplicação sobe.
