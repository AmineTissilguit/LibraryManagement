import type { Book } from "../../types";
import { Card } from "../ui/card";
import { Badge } from "../ui/badge";
import { Calendar, User, Building2, BookOpen } from "lucide-react";

interface BookCardProps {
  book: Book;
}

export default function BookCard({ book }: BookCardProps) {
  const getStatusColor = (status: Book["status"]) => {
    const numericStatus = typeof status === 'number' ? status : parseInt(status as string);
    switch (numericStatus) {
      case 0: // Available
        return "bg-green-100 text-green-800";
      case 1: // AllBorrowed
        return "bg-yellow-100 text-yellow-800";
      case 2: // Damaged
        return "bg-red-100 text-red-800";
      case 3: // Lost
        return "bg-gray-100 text-gray-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  const getStatusText = (status: Book["status"]) => {
    const numericStatus = typeof status === 'number' ? status : parseInt(status as string);
    switch (numericStatus) {
      case 0:
        return "Available";
      case 1:
        return "All Borrowed";
      case 2:
        return "Damaged";
      case 3:
        return "Lost";
      default:
        return "Unknown";
    }
  };

  return (
    <Card className="p-6 hover:shadow-lg transition-shadow">
      <div className="flex justify-between items-start mb-4">
        <div className="flex-1">
          <h3 className="text-lg font-semibold text-gray-900 mb-1">
            {book.title}
          </h3>
          <p className="text-sm text-gray-600 mb-2">ISBN: {book.isbn}</p>
        </div>
        <Badge className={getStatusColor(book.status)}>{getStatusText(book.status)}</Badge>
      </div>

      <div className="space-y-2 text-sm text-gray-600">
        <div className="flex items-center">
          <User className="h-4 w-4 mr-2" />
          <span>{book.author}</span>
        </div>

        <div className="flex items-center">
          <Building2 className="h-4 w-4 mr-2" />
          <span>{book.publisher}</span>
        </div>

        <div className="flex items-center">
          <Calendar className="h-4 w-4 mr-2" />
          <span>{book.publicationYear}</span>
        </div>

        <div className="flex items-center">
          <BookOpen className="h-4 w-4 mr-2" />
          <span>
            {book.availableCopies} of {book.totalCopies} available
          </span>
        </div>
      </div>

      <div className="mt-4 pt-4 border-t">
        <span className="inline-block bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded">
          {book.genre}
        </span>
      </div>
    </Card>
  );
}
