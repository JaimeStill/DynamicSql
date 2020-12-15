SELECT
    TOP 5 COUNT(soh.SalesOrderID) as [OrderCount], c.CustomerID, c.CompanyName
    FROM SalesLT.Customer as c
    LEFT OUTER JOIN SalesLT.SalesOrderHeader as soh
    ON c.CustomerID = soh.CustomerID
    GROUP BY
        c.CustomerId,
        c.CompanyName
    ORDER BY
        [OrderCount] DESC,
        c.CompanyName;