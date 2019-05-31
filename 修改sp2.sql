USE [jdcs]
GO
/****** Object:  StoredProcedure [dbo].[sp_g0_upd]    Script Date: 2019/3/28 ¤W¤È 09:23:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_g0_upd]
(@o_psid char(25),
@o_psname char(16),
@o_pspasswd char(25) ,  
@n_psid char(25),
@n_psname char(16), 
@n_pspasswd char(25))
  
  as
  declare @v_orgno char(20)
  select @v_orgno=op_orgno from jdcs.dbo.dcs0_org_par;
  
  delete from [g0].[dbo].[DCS0_PERSONEL]
    where ps_id = @o_psid;

  delete from g0.dbo.dcs0_org_ps
    where op_psid = @o_psid;

  insert into g0.dbo.dcs0_personel(PS_ID,PS_NAME,PS_PASSWD)
   values
   (@n_psid,@n_psname,@n_pspasswd);
  insert into g0.dbo.dcs0_org_ps(OP_PSID,OP_ORGNO)
   values
   (@n_psid,@v_orgno);