-- MileStone 1
CREATE TABLE Business (
	business_id char(22),
	Address VARCHAR NOT NULL,
	City VARCHAR NOT NULL,
	Postal_code char(5) NOT NULL,
	Latitude REAL NOT NULL,
	Longitude REAL NOT NULL,
	is_open BOOLEAN NOT NULL,
	Stars REAL NOT NULL,
	Review_count INTEGER NOT NULL,
	PRIMARY KEY(business_id)
);

CREATE TABLE User (
	user_id char(22),
	User_name VARCHAR NOT NULL,
	Average_stars REAL NOT NULL,
	Cool INTEGER NOT NULL,
	Fans INTEGER NOT NULL,
	Funny INTEGER NOT NULL,
	Tipcount INTEGER NOT NULL,
	Useful INTEGER NOT NULL,
	yelping_since DATETIME NOT NULL,
	PRIMARY KEY(user_id)
);

CREATE TABLE Tip (
	user_id char(22),
	business_id char(22),
	tip_date DATETIME,
	user_text VARCHAR,
	likes INTEGER NOT NULL,
	PRIMARY KEY(tip_date, user_id,business_id)
	FOREIGN KEY(user_id) REFERENCES User(user_id)
	FOREIGN KEY(business_id) REFERENCES Business(business_id)
);

CREATE TABLE CheckIn (
	user_id char(22),
	business_id char(22),
	checkin_date DATETIME,
	PRIMARY KEY(checkin_date, user_id,business_id)
	FOREIGN KEY(user_id) REFERENCES User(user_id)
	FOREIGN KEY(business_id) REFERENCES Business(business_id)
);

CREATE TABLE WeekHours (
	business_id char(22),
	day VARCHAR,
	hours VARCHAR, 
	PRIMARY KEY(business_id, day),
	FOREIGN KEY(business_id) REFERENCES Business(business_id)
);

CREATE TABLE addtional_attribute (
	business_id char(22),
	AA_Name VARCHAR,
	AA_Value VARCHAR,
	PRIMARY KEY(business_id, AA_Name),
	FOREIGN KEY(business_id) REFERENCES Business (business_id)
);

CREATE TABLE BusinessCategories (
	business_id char(22),
	category VARCHAR,
	PRIMARY KEY(business_id, category),
	FOREIGN KEY(business_id) REFERENCES Business(business_id)
);

CREATE TABLE friendship (
	first_user_id char(22),
	second_user_id char(22),
	PRIMARY KEY(first_user_id, second_user_id),
	FOREIGN KEY(first_user_id) REFERENCES User (user_id),
	FOREIGN KEY(second_user_id) REFERENCES User (second_user_id)
);