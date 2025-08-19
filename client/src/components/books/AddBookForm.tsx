import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  createBookSchema,
  type CreateBookFormData,
} from "../../lib/validations";
import { booksApi } from "../../lib/api";
import type { Book } from "../../types";

import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { Label } from "../ui/label";
import { Card } from "../ui/card";
import { Alert, AlertDescription } from "../ui/alert";
import { Loader2, Plus, AlertCircle, CheckCircle } from "lucide-react";

interface AddBookFormProps {
  onBookAdded: (book: Book) => void;
}

export default function AddBookForm({ onBookAdded }: AddBookFormProps) {
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<CreateBookFormData>({
    resolver: zodResolver(createBookSchema),
  });

  const onSubmit = async (data: CreateBookFormData) => {
    setError(null);
    setSuccess(null);

    try {
      const newBook = await booksApi.create(data);
      onBookAdded(newBook);
      reset();
      setSuccess("Book added successfully!");

      // Clear success message after 3 seconds
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("Failed to add book. Please try again.");
      }
    }
  };

  return (
    <Card className="p-6">
      <div className="flex items-center mb-6">
        <Plus className="h-5 w-5 mr-2" />
        <h2 className="text-lg font-semibold">Add New Book</h2>
      </div>

      {error && (
        <Alert className="mb-4 border-red-200 bg-red-50">
          <AlertCircle className="h-4 w-4 text-red-600" />
          <AlertDescription className="text-red-700">{error}</AlertDescription>
        </Alert>
      )}

      {success && (
        <Alert className="mb-4 border-green-200 bg-green-50">
          <CheckCircle className="h-4 w-4 text-green-600" />
          <AlertDescription className="text-green-700">
            {success}
          </AlertDescription>
        </Alert>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <Label htmlFor="title">Title *</Label>
            <Input
              id="title"
              {...register("title")}
              className={errors.title ? "border-red-500" : ""}
            />
            {errors.title && (
              <p className="text-sm text-red-500 mt-1">
                {errors.title.message}
              </p>
            )}
          </div>

          <div>
            <Label htmlFor="author">Author *</Label>
            <Input
              id="author"
              {...register("author")}
              className={errors.author ? "border-red-500" : ""}
            />
            {errors.author && (
              <p className="text-sm text-red-500 mt-1">
                {errors.author.message}
              </p>
            )}
          </div>

          <div>
            <Label htmlFor="isbn">ISBN *</Label>
            <Input
              id="isbn"
              placeholder="978-0-123456-78-9"
              {...register("isbn")}
              className={errors.isbn ? "border-red-500" : ""}
            />
            {errors.isbn && (
              <p className="text-sm text-red-500 mt-1">{errors.isbn.message}</p>
            )}
          </div>

          <div>
            <Label htmlFor="publisher">Publisher *</Label>
            <Input
              id="publisher"
              {...register("publisher")}
              className={errors.publisher ? "border-red-500" : ""}
            />
            {errors.publisher && (
              <p className="text-sm text-red-500 mt-1">
                {errors.publisher.message}
              </p>
            )}
          </div>

          <div>
            <Label htmlFor="publicationYear">Publication Year *</Label>
            <Input
              id="publicationYear"
              type="number"
              {...register("publicationYear", { valueAsNumber: true })}
              className={errors.publicationYear ? "border-red-500" : ""}
            />
            {errors.publicationYear && (
              <p className="text-sm text-red-500 mt-1">
                {errors.publicationYear.message}
              </p>
            )}
          </div>

          <div>
            <Label htmlFor="genre">Genre *</Label>
            <Input
              id="genre"
              placeholder="Fiction, Science, History..."
              {...register("genre")}
              className={errors.genre ? "border-red-500" : ""}
            />
            {errors.genre && (
              <p className="text-sm text-red-500 mt-1">
                {errors.genre.message}
              </p>
            )}
          </div>

          <div>
            <Label htmlFor="totalCopies">Total Copies *</Label>
            <Input
              id="totalCopies"
              type="number"
              min="1"
              {...register("totalCopies", { valueAsNumber: true })}
              className={errors.totalCopies ? "border-red-500" : ""}
            />
            {errors.totalCopies && (
              <p className="text-sm text-red-500 mt-1">
                {errors.totalCopies.message}
              </p>
            )}
          </div>
        </div>

        <Button
          type="submit"
          disabled={isSubmitting}
          className="w-full md:w-auto"
        >
          {isSubmitting ? (
            <>
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Adding Book...
            </>
          ) : (
            <>
              <Plus className="mr-2 h-4 w-4" />
              Add Book
            </>
          )}
        </Button>
      </form>
    </Card>
  );
}
