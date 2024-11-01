--
-- PostgreSQL database dump
--

-- Dumped from database version 16.4 (Debian 16.4-1.pgdg120+1)
-- Dumped by pg_dump version 16.3

-- Started on 2024-11-01 18:21:41

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 215 (class 1259 OID 16385)
-- Name: testincrement_sequence; Type: SEQUENCE; Schema: public; Owner: test_admin
--

CREATE SEQUENCE public.testincrement_sequence
    START WITH 0
    INCREMENT BY 1
    MINVALUE 0
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.testincrement_sequence OWNER TO test_admin;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 216 (class 1259 OID 16386)
-- Name: user; Type: TABLE; Schema: public; Owner: test_admin
--

CREATE TABLE public."user" (
    id integer DEFAULT nextval('public.testincrement_sequence'::regclass) NOT NULL,
    user_name text NOT NULL,
    pass_hash text NOT NULL,
    session text
);


ALTER TABLE public."user" OWNER TO test_admin;

--
-- TOC entry 3205 (class 2606 OID 16393)
-- Name: user user_pkey; Type: CONSTRAINT; Schema: public; Owner: test_admin
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_pkey PRIMARY KEY (id);


-- Completed on 2024-11-01 18:21:41

--
-- PostgreSQL database dump complete
--

