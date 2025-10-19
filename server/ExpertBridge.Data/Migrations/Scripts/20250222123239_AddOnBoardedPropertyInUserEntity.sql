START TRANSACTION;
ALTER TABLE "Users"
    ADD "IsOnBoarded" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250222123239_AddOnBoardedPropertyInUserEntity', '9.0.1');

COMMIT;

