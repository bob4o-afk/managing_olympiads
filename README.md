# managing_olympiads-
This is project for 12 class in TUES which is a service that is used for managing students that go to olympiads from school


TO DO:

!! ДА ОПРАВЯ Auth controller-а и service-а че е мазало

0. left some comments - to be check
1. like the python scrapper but for students - to add at least 10 users and to force them to change their password (with experation date)
2. to make a board for the teachers and the admins to see olymiad results
3. Fix enrollment + email - sometimes glitches and to keep track of already enrolled
4. Update profile info - only for admins
5. Да добавя liquid base (както по jacva)


6. To add:
INSERT INTO Roles (RoleName, Permissions) VALUES
('Admin', '{"create_users": true, "delete_users": true, "create_olympiads": true, "delete_olympiads": true}'),
('Teacher', '{"create_users": true, "enroll_olympiads": false}'),
('Student', '{"enroll_olympiads": true}');


more:
- created at на user-а - да се задава автоматично от backend-а като го няма
- да се задава автоматично и правата ако няма дадени някви други