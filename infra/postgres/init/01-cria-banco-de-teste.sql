-- Executado pelo PostgreSQL apenas na primeira inicialização do volume.
-- O banco connexa (desenvolvimento) é criado pela variável POSTGRES_DB.
-- Este cria o banco usado pelos testes automatizados, recriado a cada execução da suíte.
CREATE DATABASE connexa_teste OWNER connexa;
