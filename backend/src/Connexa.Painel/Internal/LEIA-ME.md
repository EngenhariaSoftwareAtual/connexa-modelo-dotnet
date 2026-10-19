# Connexa.Painel.Internal

Tudo o que é privado do módulo **Painel**: endpoints, serviços, entidades, contexto do EF Core e migrações.

- Crie as classes deste namespace como `internal` (o `ArquiteturaModularTest` confere).
- Outros módulos não podem usar nada daqui; o que eles precisarem fica no namespace raiz `Connexa.Painel`.