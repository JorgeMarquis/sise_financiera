USE FINANCIERA
GO
CREATE PROC ListarClientes
AS
SELECT * FROM Clientes
GO

CREATE PROC ObtenerCliente(
	@Id INT
)
AS
SELECT * FROM Clientes
WHERE ID = @Id
GO

CREATE PROC RegistrarCliente(
	@Apellidos	VARCHAR(50),
	@Nombres	VARCHAR(50),
	@Direccion	VARCHAR(200),
	@Telefono	VARCHAR(20),
	@Email		VARCHAR(30),
	@Tipo		INT
)
AS
INSERT INTO Clientes(Apellidos, Nombres, Direccion, Telefono, Email, TipoClienteID, Activo)
			VALUES(@Apellidos, @Nombres, @Direccion, @Telefono, @Email, @Tipo, 1)
--Retorna la cantidad de filas afectadas

SELECT @@IDENTITY --Te devuelve el último valor autogenerado
GO

CREATE PROC ListarTipoPrestamo
AS
SELECT * FROM TipoPrestamo
GO

CREATE PROC RegistrarPrestamo
(
	@fechaDeposito	DATETIME,
	@cliente		INT,
	@tipo			INT,
	@moneda			CHAR(3),
	@importe		NUMERIC(9,2),
	@plazo			INT,
	@tasa			NUMERIC(5,2)		
)
AS
INSERT INTO Prestamos(Fecha, FechaDeposito, ClienteID, TipoPrestamoID, Moneda, Importe, Plazo, Tasa, Estado)
				VALUES(GETDATE(), @fechaDeposito, @cliente, @tipo, @moneda, @importe, @plazo, @tasa, 'P')

SELECT @@IDENTITY
GO

CREATE PROC RegistrarCuotas(
	@prestamo		INT,
	@numero			INT,
	@importe		NUMERIC(9,2),
	@importeInteres	NUMERIC(9,2),
	@fechaPago		DATETIME
)
AS
INSERT INTO CuotasPrestamo(PrestamoID, Numero, Importe, ImporteInteres, FechaPago, Estado)
					VALUES(@prestamo, @numero, @importe, @importeInteres, @fechaPago, 'P')
GO

CREATE PROC ObtenerTipoClientePorID(
	@id int
)
as
select*from TiposCliente
where ID=@id
go


SELECT*FROM CuotasPrestamo