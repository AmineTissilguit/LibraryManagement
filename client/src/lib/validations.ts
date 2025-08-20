import { z } from "zod";
import { MembershipType } from "../types";

export const createBookSchema = z.object({
  isbn: z
    .string()
    .min(1, "ISBN is required")
    .regex(/^[\d-]+$/, "ISBN should only contain numbers and dashes")
    .min(10, "ISBN must be at least 10 characters")
    .max(17, "ISBN is too long"),
  title: z.string().min(1, "Title is required").max(200, "Title too long"),
  author: z
    .string()
    .min(1, "Author is required")
    .max(100, "Author name too long"),
  publisher: z
    .string()
    .min(1, "Publisher is required")
    .max(100, "Publisher name too long"),
  publicationYear: z
    .number()
    .min(1000, "Invalid year")
    .max(new Date().getFullYear(), "Year cannot be in the future"),
  genre: z.string().min(1, "Genre is required").max(50, "Genre name too long"),
  totalCopies: z
    .number()
    .min(1, "Must have at least 1 copy")
    .max(100, "Too many copies"),
});

export const createMemberSchema = z.object({
  firstName: z
    .string()
    .min(1, "First name is required")
    .max(50, "First name too long"),
  lastName: z
    .string()
    .min(1, "Last name is required")
    .max(50, "Last name too long"),
  email: z.email("Invalid email format"),
  phone: z
    .string()
    .min(1, "Phone number is required")
    .regex(
      /^(\+212|0)[567]\d{8}$/,
      "Invalid phone number. Use format: +212612345678 or 0612345678"
    ),

  address: z.string().min(1, "Address is required"),
  membershipType: z.enum(MembershipType, {
    message: "Please select a membership type",
  }),
});

export const borrowBookSchema = z.object({
  bookId: z.number().min(1, "Please select a book"),
  memberId: z.number().min(1, "Please select a member"),
});

export const returnBookSchema = z.object({
  transactionId: z.number().min(1, "Please enter a valid transaction ID"),
});

export type CreateBookFormData = z.infer<typeof createBookSchema>;
export type CreateMemberFormData = z.infer<typeof createMemberSchema>;
export type BorrowBookFormData = z.infer<typeof borrowBookSchema>;
export type ReturnBookFormData = z.infer<typeof returnBookSchema>;
