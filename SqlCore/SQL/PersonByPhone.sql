select
	c.CustomerID,
	c.LastName,
	c.FirstName,
	c.EmailAddress,
	c.Phone
from SalesLT.Customer as c
where c.Phone = '[phone]'