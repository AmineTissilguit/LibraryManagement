import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { booksApi, membersApi, transactionsApi } from "../lib/api";
import type { Book, Member } from "../types";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Card } from "../components/ui/card";
import { Alert, AlertDescription } from "../components/ui/alert";
import { Badge } from "../components/ui/badge";
import {
  History,
  Plus,
  Search,
  BookOpen,
  Users,
  CheckCircle,
  AlertCircle,
  Loader2,
  ArrowLeft,
  RotateCcw,
} from "lucide-react";
import {
  BorrowBookFormData,
  borrowBookSchema,
  ReturnBookFormData,
  returnBookSchema,
} from "../lib/validations";

export default function Transactions() {
  const [books, setBooks] = useState<Book[]>([]);
  const [members, setMembers] = useState<Member[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [showBorrowForm, setShowBorrowForm] = useState(false);
  const [showReturnForm, setShowReturnForm] = useState(false);
  const [bookSearch, setBookSearch] = useState("");
  const [memberSearch, setMemberSearch] = useState("");
  const [selectedBook, setSelectedBook] = useState<Book | null>(null);
  const [selectedMember, setSelectedMember] = useState<Member | null>(null);

  const borrowForm = useForm<BorrowBookFormData>({
    resolver: zodResolver(borrowBookSchema),
  });

  const returnForm = useForm<ReturnBookFormData>({
    resolver: zodResolver(returnBookSchema),
  });

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

  const searchBooks = async () => {
    if (!bookSearch.trim()) {
      loadBooks();
      return;
    }
    try {
      const data = await booksApi.search(bookSearch);
      setBooks(data.filter((book) => book.availableCopies > 0));
    } catch (err) {
      console.error("Book search failed:", err);
    }
  };

  const onBorrowSubmit = async (data: BorrowBookFormData) => {
    setError(null);
    setSuccess(null);

    try {
      const result = await transactionsApi.borrow({
        bookId: data.bookId,
        memberId: data.memberId,
      });

      setSuccess(`${result.message} Transaction ID: ${result.transactionId}`);
      borrowForm.reset();
      setSelectedBook(null);
      setSelectedMember(null);
      setShowBorrowForm(false);

      // Refresh books to update available copies
      loadBooks();

      setTimeout(() => setSuccess(null), 5000);
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("Failed to borrow book. Please try again.");
      }
    }
  };

  const onReturnSubmit = async (data: ReturnBookFormData) => {
    setError(null);
    setSuccess(null);

    try {
      const result = await transactionsApi.return(data.transactionId);
      setSuccess(result.message);
      returnForm.reset();
      setShowReturnForm(false);

      // Refresh books to update available copies
      loadBooks();

      setTimeout(() => setSuccess(null), 5000);
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("Failed to return book. Please try again.");
      }
    }
  };

  const handleBookSelect = (book: Book) => {
    setSelectedBook(book);
    borrowForm.setValue("bookId", book.id);
  };

  const handleMemberSelect = (member: Member) => {
    setSelectedMember(member);
    borrowForm.setValue("memberId", member.id);
  };

  const filteredMembers = members.filter(
    (member) =>
      memberSearch === "" ||
      `${member.firstName} ${member.lastName}`
        .toLowerCase()
        .includes(memberSearch.toLowerCase()) ||
      member.membershipNumber
        .toLowerCase()
        .includes(memberSearch.toLowerCase()) ||
      member.email.toLowerCase().includes(memberSearch.toLowerCase())
  );

  const closeForms = () => {
    setShowBorrowForm(false);
    setShowReturnForm(false);
    setSelectedBook(null);
    setSelectedMember(null);
    setError(null);
    borrowForm.reset();
    returnForm.reset();
  };

  useEffect(() => {
    if (showBorrowForm) {
      loadBooks();
      loadMembers();
    }
  }, [showBorrowForm]);

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
        <Card className="p-6">
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center">
              <RotateCcw className="h-5 w-5 mr-2" />
              <h2 className="text-lg font-semibold">Return Book</h2>
            </div>
            <Button variant="outline" size="sm" onClick={closeForms}>
              <ArrowLeft className="h-4 w-4 mr-2" />
              Cancel
            </Button>
          </div>

          <form
            onSubmit={returnForm.handleSubmit(onReturnSubmit)}
            className="space-y-4"
          >
            <div>
              <Label htmlFor="transactionId">Transaction ID *</Label>
              <Input
                id="transactionId"
                type="number"
                placeholder="Enter the transaction ID from when the book was borrowed"
                {...returnForm.register("transactionId", {
                  valueAsNumber: true,
                })}
                className={
                  returnForm.formState.errors.transactionId
                    ? "border-red-500"
                    : ""
                }
              />
              {returnForm.formState.errors.transactionId && (
                <p className="text-sm text-red-500 mt-1">
                  {returnForm.formState.errors.transactionId.message}
                </p>
              )}
              <p className="text-sm text-gray-600 mt-1">
                The transaction ID was provided when the book was borrowed.
                Check your borrowing receipt or records.
              </p>
            </div>

            <Button
              type="submit"
              disabled={returnForm.formState.isSubmitting}
              className="w-full"
            >
              {returnForm.formState.isSubmitting ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Processing Return...
                </>
              ) : (
                <>
                  <RotateCcw className="mr-2 h-4 w-4" />
                  Return Book
                </>
              )}
            </Button>
          </form>
        </Card>
      )}

      {/* Borrow Book Form */}
      {showBorrowForm && (
        <Card className="p-6">
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center">
              <Plus className="h-5 w-5 mr-2" />
              <h2 className="text-lg font-semibold">Borrow Book</h2>
            </div>
            <Button variant="outline" size="sm" onClick={closeForms}>
              <ArrowLeft className="h-4 w-4 mr-2" />
              Cancel
            </Button>
          </div>

          <form
            onSubmit={borrowForm.handleSubmit(onBorrowSubmit)}
            className="space-y-6"
          >
            {/* Book Selection */}
            <div className="space-y-4">
              <div>
                <Label htmlFor="bookSearch">Select Book *</Label>
                <div className="flex space-x-2">
                  <Input
                    id="bookSearch"
                    placeholder="Search books by title, author, or ISBN..."
                    value={bookSearch}
                    onChange={(e) => setBookSearch(e.target.value)}
                    onKeyPress={(e) => e.key === "Enter" && searchBooks()}
                  />
                  <Button type="button" variant="outline" onClick={searchBooks}>
                    <Search className="h-4 w-4" />
                  </Button>
                </div>
              </div>

              {selectedBook ? (
                <Card className="p-4 border-green-200 bg-green-50">
                  <div className="flex justify-between items-start">
                    <div>
                      <h4 className="font-medium text-green-800">
                        {selectedBook.title}
                      </h4>
                      <p className="text-sm text-green-600">
                        by {selectedBook.author}
                      </p>
                      <p className="text-sm text-green-600">
                        Available: {selectedBook.availableCopies} copies
                      </p>
                    </div>
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={() => {
                        setSelectedBook(null);
                        borrowForm.setValue("bookId", 0);
                      }}
                    >
                      Change
                    </Button>
                  </div>
                </Card>
              ) : (
                <div className="grid gap-2 max-h-60 overflow-y-auto">
                  {books.map((book) => (
                    <Card
                      key={book.id}
                      className="p-3 cursor-pointer hover:bg-gray-50 transition-colors"
                      onClick={() => handleBookSelect(book)}
                    >
                      <div className="flex justify-between items-center">
                        <div>
                          <h4 className="font-medium">{book.title}</h4>
                          <p className="text-sm text-gray-600">
                            by {book.author}
                          </p>
                        </div>
                        <Badge variant="outline">
                          {book.availableCopies} available
                        </Badge>
                      </div>
                    </Card>
                  ))}
                  {books.length === 0 && (
                    <p className="text-center text-gray-500 py-4">
                      No available books found. Try searching or check if books
                      are in stock.
                    </p>
                  )}
                </div>
              )}
              {borrowForm.formState.errors.bookId && (
                <p className="text-sm text-red-500">
                  {borrowForm.formState.errors.bookId.message}
                </p>
              )}
            </div>

            {/* Member Selection */}
            <div className="space-y-4">
              <div>
                <Label htmlFor="memberSearch">Select Member *</Label>
                <Input
                  id="memberSearch"
                  placeholder="Search members by name, email, or membership number..."
                  value={memberSearch}
                  onChange={(e) => setMemberSearch(e.target.value)}
                />
              </div>

              {selectedMember ? (
                <Card className="p-4 border-blue-200 bg-blue-50">
                  <div className="flex justify-between items-start">
                    <div>
                      <h4 className="font-medium text-blue-800">
                        {selectedMember.firstName} {selectedMember.lastName}
                      </h4>
                      <p className="text-sm text-blue-600">
                        {selectedMember.membershipNumber}
                      </p>
                      <p className="text-sm text-blue-600">
                        Active loans: {selectedMember.activeBorrowingsCount}
                      </p>
                    </div>
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={() => {
                        setSelectedMember(null);
                        borrowForm.setValue("memberId", 0);
                      }}
                    >
                      Change
                    </Button>
                  </div>
                </Card>
              ) : (
                <div className="grid gap-2 max-h-60 overflow-y-auto">
                  {filteredMembers.map((member) => (
                    <Card
                      key={member.id}
                      className="p-3 cursor-pointer hover:bg-gray-50 transition-colors"
                      onClick={() => handleMemberSelect(member)}
                    >
                      <div className="flex justify-between items-center">
                        <div>
                          <h4 className="font-medium">
                            {member.firstName} {member.lastName}
                          </h4>
                          <p className="text-sm text-gray-600">
                            {member.membershipNumber}
                          </p>
                        </div>
                        <div className="text-right text-sm">
                          <Badge variant="outline">
                            {member.membershipType}
                          </Badge>
                          <p className="text-gray-500 mt-1">
                            {member.activeBorrowingsCount} active loans
                          </p>
                        </div>
                      </div>
                    </Card>
                  ))}
                  {filteredMembers.length === 0 && (
                    <p className="text-center text-gray-500 py-4">
                      No active members found. Try a different search term.
                    </p>
                  )}
                </div>
              )}
              {borrowForm.formState.errors.memberId && (
                <p className="text-sm text-red-500">
                  {borrowForm.formState.errors.memberId.message}
                </p>
              )}
            </div>

            {/* Submit Button */}
            <Button
              type="submit"
              disabled={
                borrowForm.formState.isSubmitting ||
                !selectedBook ||
                !selectedMember
              }
              className="w-full"
            >
              {borrowForm.formState.isSubmitting ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Processing Loan...
                </>
              ) : (
                <>
                  <BookOpen className="mr-2 h-4 w-4" />
                  Confirm Loan
                </>
              )}
            </Button>
          </form>
        </Card>
      )}

      {/* Quick Stats */}
      <div className="grid gap-4 md:grid-cols-3">
        <Card className="p-4">
          <div className="flex items-center">
            <BookOpen className="h-8 w-8 text-blue-500 mr-3" />
            <div>
              <h3 className="font-medium">Available Books</h3>
              <p className="text-2xl font-bold text-blue-600">{books.length}</p>
            </div>
          </div>
        </Card>

        <Card className="p-4">
          <div className="flex items-center">
            <Users className="h-8 w-8 text-green-500 mr-3" />
            <div>
              <h3 className="font-medium">Active Members</h3>
              <p className="text-2xl font-bold text-green-600">
                {members.length}
              </p>
            </div>
          </div>
        </Card>

        <Card className="p-4">
          <div className="flex items-center">
            <History className="h-8 w-8 text-purple-500 mr-3" />
            <div>
              <h3 className="font-medium">Transactions</h3>
              <p className="text-2xl font-bold text-purple-600">
                Borrow & Return
              </p>
            </div>
          </div>
        </Card>
      </div>

      {/* Info Card */}
      <Card className="p-6">
        <h3 className="text-lg font-medium mb-4">Transaction Information</h3>
        <div className="grid gap-4 md:grid-cols-2">
          <div>
            <h4 className="font-medium text-gray-900 mb-2">
              Loan Periods by Membership Type:
            </h4>
            <ul className="text-sm text-gray-600 space-y-1">
              <li>• Student: 14 days (3 books max)</li>
              <li>• Adult: 21 days (5 books max)</li>
              <li>• Senior: 21 days (5 books max)</li>
              <li>• Staff: 30 days (10 books max)</li>
            </ul>
          </div>
          <div>
            <h4 className="font-medium text-gray-900 mb-2">Important Notes:</h4>
            <ul className="text-sm text-gray-600 space-y-1">
              <li>• Overdue fine: 2 MAD per day</li>
              <li>• Members with overdue books cannot borrow</li>
              <li>• Books must be available to borrow</li>
              <li>• Keep transaction ID for returns</li>
            </ul>
          </div>
        </div>
      </Card>
    </div>
  );
}
