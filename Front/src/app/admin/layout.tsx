import Header from "@/components/Header";

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="page-wrap">
      <Header />
      <main className="page-main">{children}</main>
    </div>
  );
}
