START TRANSACTION;
ALTER TABLE "ModerationReports"
    ADD "ReportedBy" text NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251101210728_AddReportedByInModerationReportEntity', '9.0.10');

COMMIT;

