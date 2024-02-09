CREATE DATABASE Estacionamiento;
USE Estacionamiento;

-- Creación de la tabla TipoVehiculo
CREATE TABLE TipoVehiculo (
    IdTipoVehiculo INT PRIMARY KEY IDENTITY(1,1),
    TipoVehiculo VARCHAR(50),
    Tarifa DECIMAL(10, 2)
);

-- Creación de la tabla Vehiculo
CREATE TABLE Vehiculo (
    NumeroPlaca VARCHAR(20) PRIMARY KEY,
    IdTipoVehiculo INT,
    FOREIGN KEY (IdTipoVehiculo) REFERENCES TipoVehiculo(IdTipoVehiculo)
);

-- Creación de la tabla Estancia
CREATE TABLE Estancia (
    IdEstancia INT PRIMARY KEY IDENTITY(1,1),
    IdVehiculo VARCHAR(20),
    HoraEntrada DATETIME,
    HoraSalida DATETIME,
    TotalPago DECIMAL(10, 2),
    FOREIGN KEY (IdVehiculo) REFERENCES Vehiculo(NumeroPlaca)
);

SELECT*FROM Estancia;
select*FROM TipoVehiculo;
SELECT*FROM VEHICULO;

--Insertando valores a la tabla TipoVehiculo--
INSERT INTO TipoVehiculo(TipoVehiculo, Tarifa) values ('Oficial', 0);
INSERT INTO TipoVehiculo(TipoVehiculo, Tarifa) values ('Residente', 0.05);
INSERT INTO TipoVehiculo(TipoVehiculo, Tarifa) values ('No residente', 0.5);

--Insertando valores a la tabla Vehiculo--
INSERT INTO Vehiculo(NumeroPlaca, IdTipoVehiculo) Values ('P123ABC', 1);
INSERT INTO Vehiculo(NumeroPlaca, IdTipoVehiculo) Values ('P176GAL', 2);
INSERT INTO Vehiculo(NumeroPlaca, IdTipoVehiculo) Values ('P518GFV', 3);
INSERT INTO Vehiculo(NumeroPlaca, IdTipoVehiculo) Values ('P492GZC', 3);

INSERT INTO Estancia(IdVehiculo, HoraEntrada,HoraSalida, TotalPago) Values('P176GAL', '2024-02-09 08:00:00', '2024-02-09 10:00:00', 0);

