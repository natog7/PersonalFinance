--INSERT INTO public."Users" ("Id", "Email", "PasswordHash", "Nickname", "Role", "IsActive", "CreatedAt")
--VALUES (uuidv7(), 'admin@email.com', '$2a$12$M4cKsLHjk7nLnDO0WEMdjuByxjSRCFGSw6YwGpJq8nlVKivA4uMzG', 'admin', 'Admin', true, now())

SELECT * FROM "Users"