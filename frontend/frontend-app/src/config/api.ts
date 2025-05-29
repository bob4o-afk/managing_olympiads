export const API_BASE_URL = `${process.env.REACT_APP_API_URL}/api`;

export const API_ROUTES = {
  login: `${API_BASE_URL}/auth/login`,
  validatePassword: `${API_BASE_URL}/auth/validate-password`,
  validateToken: `${API_BASE_URL}/auth/validate-token`,
  requestPasswordReset: `${API_BASE_URL}/auth/request-password-change`,
  resetPassword: `${API_BASE_URL}/auth/reset-password`,
  userRoleAssignment: `${API_BASE_URL}/user-role-assignments`,
  olympiads: `${API_BASE_URL}/olympiads`,
  academicYears: `${API_BASE_URL}/academic-years`,
  sendEmail: `${API_BASE_URL}/email/send`,
  sendEmailWithDocument: `${API_BASE_URL}/email/send-document`,
  studentOlympiadEnrollment: `${API_BASE_URL}/student-olympiad-enrollments`,
  studentOlympiadEnrollmentById: (enrollmentId: string) =>
    `${API_BASE_URL}/student-olympiad-enrollments/${enrollmentId}`,
  roles: `${API_BASE_URL}/roles`,
  users: `${API_BASE_URL}/users`,
  userById: (userId: string) => `${API_BASE_URL}/users/${userId}`,
  userEnrollments: (userId: string) =>
    `${API_BASE_URL}/student-olympiad-enrollments/user/${userId}`,
  resetPasswordWithToken: (token: string) =>
    `${API_BASE_URL}/auth/reset-password?token=${encodeURIComponent(token)}`,
};
