"use client";
import useSWR from "swr";
import { apiClient } from "@/lib/apiClient";
import type { LoanResponse } from "@/lib/types";

const KEY = "my-loans";

export function useMyLoans() {
  const { data, error, isLoading, mutate } = useSWR<LoanResponse[]>(
    KEY,
    () => apiClient.get<LoanResponse[]>("/api/v1/Loans/my")
  );
  return { loans: data ?? [], error, isLoading, mutate };
}
