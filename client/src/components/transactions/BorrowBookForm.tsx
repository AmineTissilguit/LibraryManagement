import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  BorrowBookFormData,
  borrowBookSchema,
} from "../../lib/validations";
import { booksApi, membersApi, transactionsApi } from "../../lib/api";
import type { Book, Member } from "../../types";

import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { Label } from "../ui/label";
import { Card } from "../ui/card";
import { Alert, AlertDescription } from "../ui/alert";
import { Badge } from "../ui/badge";
import { getMembershipTypeLabel, getMembershipTypeBadgeVariant } from "../../lib/utils";
import {
  Plus,
  Search,
  BookOpen,
  ArrowLeft,
  Loader2,
  AlertCircle,
  CheckCircle,
} from "lucide-react";

interface BorrowBookFormProps {
  onTransactionSuccess: (message: string) => void;
  onCancel: () => void;
}

export default function BorrowBookForm({ onTransactionSuccess, onCancel }: BorrowBookFormProps) {
  const [books, setBooks] = useState<Book[]>([]);
  const [members, setMembers] = useState<Member[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [bookSearch, setBookSearch] = useState("");
  const [memberSearch, setMemberSearch] = useState("");
  const [selectedBook, setSelectedBook] = useState<Book | null>(null);
  const [selectedMember, setSelectedMember] = useState<Member | null>(null);

  const {
    handleSubmit,
    setValue,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<BorrowBookFormData>({
    resolver: zodResolver(borrowBookSchema),
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

  const onSubmit = async (data: BorrowBookFormData) => {
    setError(null);

    try {
      const result = await transactionsApi.borrow({
        bookId: data.bookId,
        memberId: data.memberId,
      });

      onTransactionSuccess(`${result.message} Transaction ID: ${result.transactionId}`);
      reset();
      setSelectedBook(null);
      setSelectedMember(null);
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("Failed to borrow book. Please try again.");
      }
    }
  };

  const handleBookSelect = (book: Book) => {
    setSelectedBook(book);
    setValue("bookId", book.id);
  };

  const handleMemberSelect = (member: Member) => {
    setSelectedMember(member);
    setValue("memberId", member.id);
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
      member.email.toLowerCase().includes(memberSearch.toLowerCase()) ||
      getMembershipTypeLabel(member.membershipType)
        .toLowerCase()
        .includes(memberSearch.toLowerCase())
  );

  useEffect(() => {
    loadBooks();
    loadMembers();
  }, []);

  return (
    <Card className="p-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center">
          <Plus className="h-5 w-5 mr-2" />
          <h2 className="text-lg font-semibold">Borrow Book</h2>
        </div>
        <Button variant="outline" size="sm" onClick={onCancel}>
          <ArrowLeft className="h-4 w-4 mr-2" />
          Cancel
        </Button>
      </div>

      {error && (
        <Alert className="mb-4 border-red-200 bg-red-50">
          <AlertCircle className="h-4 w-4 text-red-600" />
          <AlertDescription className="text-red-700">{error}</AlertDescription>
        </Alert>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
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
                    setValue("bookId", 0);
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
          {errors.bookId && (
            <p className="text-sm text-red-500">
              {errors.bookId.message}
            </p>
          )}
        </div>

        {/* Member Selection */}
        <div className="space-y-4">
          <div>
            <Label htmlFor="memberSearch">Select Member *</Label>
            <Input
              id="memberSearch"
              placeholder="Search members by name, email, membership number, or member type..."
              value={memberSearch}
              onChange={(e) => setMemberSearch(e.target.value)}
            />
          </div>

          {selectedMember ? (
            <Card className="p-4 border-blue-200 bg-blue-50">
              <div className="flex justify-between items-start">
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <h4 className="font-medium text-blue-800">
                      {selectedMember.firstName} {selectedMember.lastName}
                    </h4>
                    <Badge variant={getMembershipTypeBadgeVariant(selectedMember.membershipType)} size="sm">
                      {getMembershipTypeLabel(selectedMember.membershipType)}
                    </Badge>
                  </div>
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
                    setValue("memberId", 0);
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
                      <Badge variant={getMembershipTypeBadgeVariant(member.membershipType)}>
                        {getMembershipTypeLabel(member.membershipType)}
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
          {errors.memberId && (
            <p className="text-sm text-red-500">
              {errors.memberId.message}
            </p>
          )}
        </div>

        {/* Submit Button */}
        <Button
          type="submit"
          disabled={
            isSubmitting ||
            !selectedBook ||
            !selectedMember
          }
          className="w-full"
        >
          {isSubmitting ? (
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
  );
}