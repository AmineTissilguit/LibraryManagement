import { Outlet, Link, useLocation } from "react-router-dom";
import { BookOpen, Users, History } from "lucide-react";
import { Button } from "../ui/button";

export default function Layout() {
  const location = useLocation();

  const isActive = (path: string) =>
    location.pathname === path ||
    (path === "/books" && location.pathname === "/");

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center space-x-2">
              <BookOpen className="h-8 w-8 text-blue-600" />
              <h1 className="text-xl font-bold text-gray-900">
                Library Management System
              </h1>
            </div>
          </div>
        </div>
      </header>

      <div className="flex">
        {/* Sidebar */}
        <nav className="w-64 bg-white shadow-sm min-h-screen border-r">
          <div className="p-4 space-y-2">
            <Button
              asChild
              variant={
                isActive("/books") || isActive("/") ? "default" : "ghost"
              }
              className="w-full justify-start"
            >
              <Link to="/books">
                <BookOpen className="mr-3 h-4 w-4" />
                Books
              </Link>
            </Button>

            <Button
              asChild
              variant={isActive("/members") ? "default" : "ghost"}
              className="w-full justify-start"
            >
              <Link to="/members">
                <Users className="mr-3 h-4 w-4" />
                Members
              </Link>
            </Button>

            <Button
              asChild
              variant={isActive("/transactions") ? "default" : "ghost"}
              className="w-full justify-start"
            >
              <Link to="/transactions">
                <History className="mr-3 h-4 w-4" />
                Transactions
              </Link>
            </Button>
          </div>
        </nav>

        {/* Main Content */}
        <main className="flex-1 p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
