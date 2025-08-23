import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  createMemberSchema,
  type CreateMemberFormData,
} from "../../lib/validations";
import { membersApi } from "../../lib/api";
import type { Member } from "../../types";
import { MembershipType } from "../../types";

import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { Label } from "../ui/label";
import { Card } from "../ui/card";
import { Alert, AlertDescription } from "../ui/alert";
import { Loader2, Plus, AlertCircle, CheckCircle } from "lucide-react";

interface AddMemberFormProps {
  onMemberAdded: (member: Member) => void;
  onCancel: () => void;
}

export default function AddMemberForm({ onMemberAdded, onCancel }: AddMemberFormProps) {
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<CreateMemberFormData>({
    resolver: zodResolver(createMemberSchema),
  });

  const onSubmit = async (data: CreateMemberFormData) => {
    setError(null);
    setSuccess(null);

    try {
      const newMember = await membersApi.create(data);
      onMemberAdded(newMember);
      reset();
      setSuccess(
        `Member registered successfully! Membership Number: ${newMember.membershipNumber}`
      );

      setTimeout(() => setSuccess(null), 5000);
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("Failed to register member. Please try again.");
      }
    }
  };

  return (
    <Card className="p-6">
      <div className="flex items-center mb-6">
        <Plus className="h-5 w-5 mr-2" />
        <h2 className="text-lg font-semibold">Register New Member</h2>
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
            <Label htmlFor="firstName">First Name *</Label>
            <Input
              id="firstName"
              {...register("firstName")}
              className={errors.firstName ? "border-red-500" : ""}
            />
            {errors.firstName && (
              <p className="text-sm text-red-500 mt-1">
                {errors.firstName.message}
              </p>
            )}
          </div>

          <div>
            <Label htmlFor="lastName">Last Name *</Label>
            <Input
              id="lastName"
              {...register("lastName")}
              className={errors.lastName ? "border-red-500" : ""}
            />
            {errors.lastName && (
              <p className="text-sm text-red-500 mt-1">
                {errors.lastName.message}
              </p>
            )}
          </div>

          <div>
            <Label htmlFor="email">Email *</Label>
            <Input
              id="email"
              type="email"
              {...register("email")}
              className={errors.email ? "border-red-500" : ""}
            />
            {errors.email && (
              <p className="text-sm text-red-500 mt-1">
                {errors.email.message}
              </p>
            )}
          </div>

          <div>
            <Label htmlFor="phone">Phone Number *</Label>
            <Input
              id="phone"
              placeholder="+212612345678 or 0612345678"
              {...register("phone")}
              className={errors.phone ? "border-red-500" : ""}
            />
            {errors.phone && (
              <p className="text-sm text-red-500 mt-1">
                {errors.phone.message}
              </p>
            )}
          </div>

          <div className="md:col-span-2">
            <Label htmlFor="address">Address *</Label>
            <Input
              id="address"
              {...register("address")}
              className={errors.address ? "border-red-500" : ""}
            />
            {errors.address && (
              <p className="text-sm text-red-500 mt-1">
                {errors.address.message}
              </p>
            )}
          </div>

          <div>
            <Label htmlFor="membershipType">Membership Type *</Label>
            <select
              id="membershipType"
              {...register("membershipType", { valueAsNumber: true })}
              className={`flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background ${
                errors.membershipType ? "border-red-500" : ""
              }`}
            >
              <option value="">Select membership type</option>
              <option value={MembershipType.Student}>
                Student (3 books, 14 days)
              </option>
              <option value={MembershipType.Adult}>
                Adult (5 books, 21 days)
              </option>
              <option value={MembershipType.Senior}>
                Senior (5 books, 21 days)
              </option>
              <option value={MembershipType.Staff}>
                Staff (10 books, 30 days)
              </option>
            </select>
            {errors.membershipType && (
              <p className="text-sm text-red-500 mt-1">
                {errors.membershipType.message}
              </p>
            )}
          </div>
        </div>

        <div className="flex space-x-2">
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Registering...
              </>
            ) : (
              <>
                <Plus className="mr-2 h-4 w-4" />
                Register Member
              </>
            )}
          </Button>
          <Button
            type="button"
            variant="outline"
            onClick={onCancel}
          >
            Cancel
          </Button>
        </div>
      </form>
    </Card>
  );
}