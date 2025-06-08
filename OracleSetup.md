# Oracle Database Setup

Execute the following SQL statements on your Oracle server to create the tables used by the API.

```sql
CREATE TABLE SENSORES (
    ID              VARCHAR2(36) PRIMARY KEY,
    LOCALIZACAO     VARCHAR2(100),
    TIPO            VARCHAR2(50),
    STATUS          VARCHAR2(50)
);

CREATE TABLE LEITURAS (
    ID                 VARCHAR2(36) PRIMARY KEY,
    SENSOR_ID          VARCHAR2(36) REFERENCES SENSORES(ID),
    NIVEL_AGUA         NUMBER,
    VELOCIDADE_CHUVA   NUMBER,
    TIMESTAMP_LEITURA  TIMESTAMP
);

CREATE TABLE ALERTAS (
    ID                VARCHAR2(36) PRIMARY KEY,
    LOCAL             VARCHAR2(100),
    NIVEL_RISCO       VARCHAR2(20),
    ACAO_RECOMENDADA  VARCHAR2(200)
);

CREATE TABLE USUARIOS (
    ID     VARCHAR2(36) PRIMARY KEY,
    NOME   VARCHAR2(100),
    EMAIL  VARCHAR2(100),
    ROLE   VARCHAR2(50)
);
```

The API expects a connection string similar to the one defined in `appsettings.json`:

```
User Id=rm554199;Password=160103;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SID=orcl)))
```

Make sure the user `rm554199` has privileges to create and read these tables.
