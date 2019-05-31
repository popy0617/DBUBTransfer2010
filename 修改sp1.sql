USE [jdcs]
GO
/****** Object:  StoredProcedure [dbo].[sp_g0_ins]    Script Date: 2019/3/28 ¤W¤È 09:17:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[sp_g0_ins]
@psid VARCHAR(25) ,
@psname VARCHAR(16), 
@pspasswd VARCHAR(25)

as
  declare @v_orgno VARCHAR(20) ;
  select @v_orgno=op_orgno from jdcs..dcs0_org_par;

  delete from g0..dcs0_personel
    where ps_id = @psid;

  delete from g0..dcs0_org_ps
    where op_psid = @psid;
    
  insert into g0..dcs0_personel(PS_ID,PS_NAME,PS_PASSWD)
   values
   (@psid,@psname,@pspasswd);
  insert into g0..dcs0_org_ps(OP_PSID,OP_ORGNO)
   values
   (@psid,@v_orgno);