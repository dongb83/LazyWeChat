create table records
(
	id nvarchar(100) primary key,
    messageObject nvarchar(5000),
    createdAt datetime,
    status int
)

