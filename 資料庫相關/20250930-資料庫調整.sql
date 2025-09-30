-- public.tb_control_log definition

-- Drop table

-- DROP TABLE tb_control_log;

CREATE TABLE tb_control_log (
	id int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	logtime timestamp NOT NULL,
	ip varchar(50) NULL,
	"account" varchar(100) NULL,
	"name" varchar(100) NULL,
	"exception" text NULL,
	status varchar(50) NULL,
	controllername varchar(100) NULL,
	actionname varchar(100) NULL,
	"token" varchar(8000) NULL, -- JWT Token
	CONSTRAINT tb_controllog_pkey PRIMARY KEY (id)
);

-- Column comments

COMMENT ON COLUMN tb_control_log."token" IS 'JWT Token';

-- public.tb_token definition

-- Drop table

-- DROP TABLE tb_token;

CREATE TABLE tb_token (
	tokenid varchar(8000) NOT NULL, -- 金鑰ID
	"account" varchar(100) NOT NULL, -- 使用帳號
	status int4 NOT NULL,
	registerdate timestamp NULL,
	voidstarttime timestamp NULL,
	voidendtime timestamp NULL,
	createtime timestamp NULL,
	updatetime timestamp NULL
);
COMMENT ON TABLE tb_token IS '金鑰資料表';

-- Column comments

COMMENT ON COLUMN tb_token.tokenid IS '金鑰ID';
COMMENT ON COLUMN tb_token."account" IS '使用帳號';
