CREATE INDEX [I1_ACER_LOG]  ON [dbo].[DCS0_ACER_LOG] (AL_WORD, AL_SNO);
CREATE INDEX [I1_CURT_SNO] ON [dbo].[DCS0_CURT_SNO] (CS_FROM);
CREATE UNIQUE INDEX I1_EXG_SERVER ON DCS0_EXG_SERVER(ES_NAME);
CREATE UNIQUE INDEX I1_PROCESS ON DCS0_PROCESS (PC_NAME);
CREATE INDEX I2_ANNO ON DCS1_ANNO(AN_TO,AN_STDT,AN_ENDDT);
CREATE INDEX I3_EMAL_TMP ON DCS1_EMAL_TMP (EM_TO,EM_FROM,EM_DID,EM_SNO);
CREATE INDEX I6_EMAL_TMP ON DCS1_EMAL_TMP (EM_PFLAG,EM_FROM,EM_DATE,EM_OBJ,EM_RDATE);
CREATE UNIQUE INDEX  I1_GCA_MAST  ON DCS2_GCA_MAST(GM_FROM,GM_WORD,GM_SNO);
CREATE UNIQUE INDEX I1_GCA_REC  ON DCS2_GCA_REC(GR_FROM,GR_WORD,GR_SNO,GR_LN);
CREATE UNIQUE INDEX I1_ISDP_REC ON DCS2_ISDP_REC (IR_FROM,IR_WORD,IR_SNO,IR_LN);
CREATE INDEX I2_OUT_MAST ON DCS2_OUT_MAST (OM_WORD,OM_SNO);
CREATE INDEX I1_WRAP_REC ON DCS2_WRAP_REC(WR_TDATE,WR_SNO,WR_WORD,WR_TYPE);
CREATE INDEX I2_WRAP_REC ON DCS2_WRAP_REC(WR_PDATE,WR_DPNA);
CREATE INDEX  I1_CUST_DAT  ON DCS3_CUST_DAT(CD_WORD,CD_SNO,CD_TYPE);
CREATE UNIQUE INDEX I1_CUST_FRM ON DCS3_CUST_FRM (CF_FROM,CF_WORD,CF_SNO);
CREATE UNIQUE INDEX I1_CUST_REC ON DCS3_CUST_REC(CR_FROM,CR_WORD,CR_SNO,CR_LN);
CREATE UNIQUE INDEX I1_FILE_IDX ON DCS5_FILE_IDX(FI_TO,FI_FTYPE,FI_ORG_NO,FI_WORD,FI_SNO);
CREATE INDEX I3_FILE_IDX ON DCS5_FILE_IDX(FI_TO,FI_FTYPE,FI_WORD,FI_SNO);