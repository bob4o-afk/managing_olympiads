export interface StudentOlympiadEnrollmentData{
  userId: string;
  olympiadId: string;
  academicYearId: string;
  enrollmentStatus: "pending" | "approved" | "rejected";
  createdAt: string;
}
