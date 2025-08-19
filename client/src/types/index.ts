export interface Book {
  id: number;
  isbn: string;
  title: string;
  author: string;
  publisher: string;
  publicationYear: number;
  genre: string;
  totalCopies: number;
  availableCopies: number;
  status: "Available" | "AllBorrowed" | "Damaged" | "Lost";
  createdAt: string;
}

export interface Member {
  id: number;
  membershipNumber: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address: string;
  membershipType: "Student" | "Adult" | "Senior" | "Staff";
  isActive: boolean;
  activeBorrowingsCount: number;
  registrationDate: string;
  createdAt: string;
}

export interface BorrowingTransaction {
  id: number;
  bookId: number;
  memberId: number;
  borrowDate: string;
  dueDate: string;
  returnDate?: string;
  status: "Active" | "Returned" | "Overdue";
  fineAmount: number;
  createdAt: string;
}

export interface CreateBookRequest {
  isbn: string;
  title: string;
  author: string;
  publisher: string;
  publicationYear: number;
  genre: string;
  totalCopies: number;
}

export interface CreateMemberRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address: string;
  membershipType: "Student" | "Adult" | "Senior" | "Staff";
}

export interface BorrowBookRequest {
  bookId: number;
  memberId: number;
}
