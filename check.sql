USE DB_LOCADORA;
DELETE FROM Emprestimos;
DELETE FROM JogoDatasLancamento;
DELETE FROM JogoDesenvolvedor;
DELETE FROM JogoGenero;
DELETE FROM JogoPublicadora;
DELETE FROM Jogos;
SELECT COUNT(*) AS TotalJogosAposLimpeza FROM Jogos;
