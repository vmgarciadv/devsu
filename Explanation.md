# Reportes Endpoint - Optimized Implementation

## Requirements
The `/reportes` endpoint needs to show account-level summaries with:
- **Fecha**: Date range of the report
- **Cliente**: Client name
- **NumeroCuenta**: Account number
- **TipoCuenta**: Account type
- **SaldoInicial**: Initial account balance
- **Estado**: Account status
- **Movimiento**: Total sum of all movements for the account
- **SaldoDisponible**: Current available balance (SaldoInicial + sum of movements)

## Chosen Solution: Stored Procedure with Optimized Indexes

### Why Stored Procedure is Most Optimized:

1. **Pre-compiled execution plan**: SQL Server compiles and caches the execution plan
2. **Optimal performance**: Can use SQL Server's query optimizer effectively
3. **Reduced network traffic**: Only parameters sent, not the entire query
4. **Can leverage indexes**: We can create specific indexes for this query pattern
5. **Parameterized queries**: Prevents SQL injection and improves plan reuse

### Performance Optimizations:

#### 1. Indexes for Optimization:
```sql
-- Index for fast client lookup
CREATE INDEX IX_Clientes_Nombre ON Clientes(Nombre) INCLUDE (Id);

-- Index for movement date filtering
CREATE INDEX IX_Movimientos_CuentaId_Fecha ON Movimientos(CuentaId, Fecha) INCLUDE (Valor);

-- Index for account-client relationship
CREATE INDEX IX_Cuentas_ClienteId ON Cuentas(ClienteId) INCLUDE (NumeroCuenta, TipoCuenta, SaldoInicial, Estado);
```

#### 2. Stored Procedure:
```sql
CREATE PROCEDURE sp_GenerarReporteEstadoCuenta
    @ClienteNombre NVARCHAR(100),
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        @FechaInicio as FechaInicio,
        @FechaFin as FechaFin,
        c.Nombre as Cliente,
        cu.NumeroCuenta,
        cu.TipoCuenta,
        cu.SaldoInicial,
        cu.Estado,
        ISNULL(SUM(m.Valor), 0) as TotalMovimientos,
        cu.SaldoInicial + ISNULL(SUM(m.Valor), 0) as SaldoDisponible
    FROM Cuentas cu
    INNER JOIN Clientes c ON cu.ClienteId = c.Id
    LEFT JOIN Movimientos m ON cu.CuentaId = m.CuentaId 
        AND m.Fecha >= @FechaInicio 
        AND m.Fecha <= @FechaFin
    WHERE c.Nombre = @ClienteNombre
    GROUP BY c.Id, c.Nombre, cu.CuentaId, cu.NumeroCuenta, 
             cu.TipoCuenta, cu.SaldoInicial, cu.Estado
    ORDER BY cu.NumeroCuenta;
END
```

### Expected Performance Benefits:
- **Sub-millisecond response times** for most queries
- **Scalability** as data grows
- **Consistent performance** due to cached execution plans
- **Reduced database load** through efficient indexing and query optimization

### Implementation Components:
1. Database migration for indexes and stored procedure
2. ReporteDto model for the response
3. ReportesController with GET endpoint
4. ReporteService and Repository for stored procedure execution
5. Error handling for invalid parameters