export interface LoanResponse {
  id: string;
  userId: string;
  userName: string;
  amount: number;
  termInMonths: number;
  purpose: string;
  status: "Pending" | "Approved" | "Rejected";
  createdAt: string;
  approvedAt: string | null;
  rejectedAt: string | null;
  rejectionReason: string | null;
}

export interface AuthResponse {
  token: string;
  email: string;
  fullName: string;
  role: string;
}
