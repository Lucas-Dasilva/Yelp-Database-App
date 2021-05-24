-- 5.a
Create Or REPLACE Function  NumTipUpdate() Returns trigger As '
Begin
    Update business
    Set num_tips =  num_tips + 1
    Where business.business_id = New.business_id;
    Return New;
End
' Language plpgsql;

create Trigger NumTipUpdate
    After Insert On Tip
    For Each Row
    When (New.business_id Is Not NULL)
    Execute Procedure NumTipUpdate();

Create Or Replace Function  TipCountUpdate() Returns trigger As '
Begin
    Update UserTable
    Set tipcount = tipcount + 1
    Where UserTable.user_id = New.user_id;
    Return New;
End
' Language plpgsql;

create Trigger TipCountUpdate
    After Insert On Tip
    For Each Row
    When (New.user_id Is Not NULL)
    Execute Procedure TipCountUpdate();


--5.b
Create Or Replace Function  CheckInUpdate() Returns trigger As '
Begin
    Update Business
    Set num_checkins = num_checkins + 1
    Where Business.business_id = New.business_id;
    Return New;
End
' Language plpgsql;

create Trigger CheckInUpdate
    After Insert On CheckIn
    For Each Row
    When (New.business_id Is Not Null)
    Execute Procedure CheckInUpdate();

--5.c
Create Or Replace Function  TotalLikeUser() Returns trigger As '
Begin
    Update UserTable
    Set total_likes = total_likes + 1
    Where UserTable.user_id = New.user_id And (Old.likes != New.likes and New.date = Old.date);
    Return New;
End
' Language plpgsql;

create Trigger TotalLikeUser
    After Update on tip
    For Each Row
    When (New.user_id Is Not Null)
    Execute Procedure TotalLikeUser();




--Test For 5.a
INSERT INTO Tip values ('---1lKK3aKOuomHnwAkAow', '0zoXYHq82haayMaV952jEQ', '2018-06-26 20:42:10', 'Good stuff, I liked it.', 0);

SELECT * FROM Tip Where user_id = '---1lKK3aKOuomHnwAkAow' AND business_id = '0zoXYHq82haayMaV952jEQ';
--- num_tips should increment by 1
Select * From business Where business_id = '0zoXYHq82haayMaV952jEQ';
--- tipCount should increment by 1
Select * From UserTable where user_id = '---1lKK3aKOuomHnwAkAow';



--Test For 5.b
Insert Into CheckIn Values ('gnKjwL_1w79qoiV3IC_xQQ', '2011-12-26 01:46:17');

SELECT * FROM CheckIn WHERE business_id = 'gnKjwL_1w79qoiV3IC_xQQ' AND checkin_date = '2011-12-26 01:46:17';
--- checkin should increment by 1
Select * From business Where business_id = 'gnKjwL_1w79qoiV3IC_xQQ';



--Test For 5.c
UPDATE Tip SET likes = likes + 1 WHERE user_id = 'jRyO2V1pA4CdVVqCIOPc1Q' AND business_id = '5KheTjYPu1HcQzQFtm4_vw' AND date = '2011-12-26 01:46:17';

--- likes should increment by 1
select * from tip
WHERE user_id = 'jRyO2V1pA4CdVVqCIOPc1Q'
AND business_id = '5KheTjYPu1HcQzQFtm4_vw'
AND date = '2011-12-26 01:46:17';

--- likes should increment by 1
select * from usertable
WHERE user_id = 'jRyO2V1pA4CdVVqCIOPc1Q';


--- need to delete these observations if already run test 5a and 5b
--DELETE FROM Tip WHERE user_id = '---1lKK3aKOuomHnwAkAow' AND business_id = '0zoXYHq82haayMaV952jEQ' AND date = '2018-06-26 20:42:10';
--DELETE FROM CheckIn WHERE business_id = 'gnKjwL_1w79qoiV3IC_xQQ' AND checkin_date = '2011-12-26 01:46:17';