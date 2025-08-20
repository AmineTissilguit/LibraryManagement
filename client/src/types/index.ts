export enum MembershipType {
  Student = 0,
  Adult = 1,
  Senior = 2,
  Staff = 3,
}

export enum BookStatus {
  Available = "Available",
  AllBorrowed = "AllBorrowed",
  Damaged = "Damaged",
  Lost = "Lost",
}

export enum TransactionStatus {
  Active = "Active",
  Returned = "Returned",
  Overdue = "Overdue",
}

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
  status: BookStatus;
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
  membershipType: MembershipType;
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
  status: TransactionStatus;
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
  membershipType: MembershipType;
}

export interface BorrowBookRequest {
  bookId: number;
  memberId: number;
}
