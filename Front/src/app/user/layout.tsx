import Header from "@/components/Header";

export default function UserLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="page-wrap">
      <Header />
      <main className="page-main-sm">{children}</main>
    </div>
  );
}
