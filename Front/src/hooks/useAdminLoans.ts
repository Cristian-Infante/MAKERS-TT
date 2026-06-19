"use client";
import useSWR from "swr";
import { apiClient } from "@/lib/apiClient";
import type { LoanResponse } from "@/lib/types";

const KEY = "admin-loans";

export function useAdminLoans() {
  const { data, error, isLoading, mutate } = useSWR<LoanResponse[]>(
    KEY,
    () => apiClient.get<LoanResponse[]>("/api/v1/Loans/admin")
  );
  return { loans: data ?? [], error, isLoading, mutate };
}
