ALTER TABLE control_log ADD token varchar(8000) NULL;

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
