import { z } from "zod";

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

export type CreateBookFormData = z.infer<typeof createBookSchema>;
