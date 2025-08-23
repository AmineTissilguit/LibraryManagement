import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  ReturnBookFormData,
  returnBookSchema,
} from "../../lib/validations";
import { transactionsApi } from "../../lib/api";

import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { Label } from "../ui/label";
import { Card } from "../ui/card";
import { Alert, AlertDescription } from "../ui/alert";
import {
  RotateCcw,
  ArrowLeft,
  Loader2,
  AlertCircle,
} from "lucide-react";

interface ReturnBookFormProps {
  onTransactionSuccess: (message: string) => void;
  onCancel: () => void;
}

export default function ReturnBookForm({ onTransactionSuccess, onCancel }: ReturnBookFormProps) {
  const [error, setError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<ReturnBookFormData>({
    resolver: zodResolver(returnBookSchema),
  });

  const onSubmit = async (data: ReturnBookFormData) => {
    setError(null);

    try {
      const result = await transactionsApi.return(data.transactionId);
      onTransactionSuccess(result.message);
      reset();
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("Failed to return book. Please try again.");
      }
    }
  };

  return (
    <Card className="p-6">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center">
          <RotateCcw className="h-5 w-5 mr-2" />
          <h2 className="text-lg font-semibold">Return Book</h2>
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

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <Label htmlFor="transactionId">Transaction ID *</Label>
          <Input
            id="transactionId"
            type="number"
            placeholder="Enter the transaction ID from when the book was borrowed"
            {...register("transactionId", {
              valueAsNumber: true,
            })}
            className={errors.transactionId ? "border-red-500" : ""}
          />
          {errors.transactionId && (
            <p className="text-sm text-red-500 mt-1">
              {errors.transactionId.message}
            </p>
          )}
          <p className="text-sm text-gray-600 mt-1">
            The transaction ID was provided when the book was borrowed.
            Check your borrowing receipt or records.
          </p>
        </div>

        <Button
          type="submit"
          disabled={isSubmitting}
          className="w-full"
        >
          {isSubmitting ? (
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
  );
}