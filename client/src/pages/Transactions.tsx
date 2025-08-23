import { useState, useEffect } from "react";
import { booksApi, membersApi } from "../lib/api";
import type { Book, Member } from "../types";
import BorrowBookForm from "../components/transactions/BorrowBookForm";
import ReturnBookForm from "../components/transactions/ReturnBookForm";
import TransactionStats from "../components/transactions/TransactionStats";
import TransactionInfo from "../components/transactions/TransactionInfo";
import { Button } from "../components/ui/button";
import { Alert, AlertDescription } from "../components/ui/alert";
import {
  Plus,
  CheckCircle,
  AlertCircle,
  RotateCcw,
} from "lucide-react";

export default function Transactions() {
  const [books, setBooks] = useState<Book[]>([]);
  const [members, setMembers] = useState<Member[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [showBorrowForm, setShowBorrowForm] = useState(false);
  const [showReturnForm, setShowReturnForm] = useState(false);

  const loadBooks = async () => {
    try {
      const data = await booksApi.getAll();
      setBooks(data.filter((book) => book.availableCopies > 0));
    } catch (err) {
      console.error("Failed to load books:", err);
    }
  };

  const loadMembers = async () => {
    try {
      const data = await membersApi.getAll();
      setMembers(data.filter((member) => member.isActive));
    } catch (err) {
      console.error("Failed to load members:", err);
    }
  };

  const handleTransactionSuccess = (message: string) => {
    setSuccess(message);
    setShowBorrowForm(false);
    setShowReturnForm(false);
    loadBooks(); // Refresh books to update available copies
    setTimeout(() => setSuccess(null), 5000);
  };

  const closeForms = () => {
    setShowBorrowForm(false);
    setShowReturnForm(false);
    setError(null);
  };

  useEffect(() => {
    loadBooks();
    loadMembers();
  }, []);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Transactions</h1>
          <p className="text-muted-foreground">
            Manage book borrowing and returns
          </p>
        </div>
        <div className="flex space-x-2">
          <Button
            variant="outline"
            onClick={() => {
              closeForms();
              setShowReturnForm(true);
            }}
          >
            <RotateCcw className="mr-2 h-4 w-4" />
            Return Book
          </Button>
          <Button
            onClick={() => {
              closeForms();
              setShowBorrowForm(true);
            }}
          >
            <Plus className="mr-2 h-4 w-4" />
            Borrow Book
          </Button>
        </div>
      </div>

      {/* Success/Error Messages */}
      {error && (
        <Alert className="border-red-200 bg-red-50">
          <AlertCircle className="h-4 w-4 text-red-600" />
          <AlertDescription className="text-red-700">{error}</AlertDescription>
        </Alert>
      )}

      {success && (
        <Alert className="border-green-200 bg-green-50">
          <CheckCircle className="h-4 w-4 text-green-600" />
          <AlertDescription className="text-green-700">
            {success}
          </AlertDescription>
        </Alert>
      )}

      {/* Return Book Form */}
      {showReturnForm && (
        <ReturnBookForm 
          onTransactionSuccess={handleTransactionSuccess}
          onCancel={closeForms}
        />
      )}

      {/* Borrow Book Form */}
      {showBorrowForm && (
        <BorrowBookForm 
          onTransactionSuccess={handleTransactionSuccess}
          onCancel={closeForms}
        />
      )}

      {/* Quick Stats */}
      <TransactionStats 
        availableBooks={books.length}
        activeMembers={members.length}
      />

      {/* Info Card */}
      <TransactionInfo />
    </div>
  );
}
