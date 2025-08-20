import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  createMemberSchema,
  type CreateMemberFormData,
} from "../lib/validations";
import { membersApi } from "../lib/api";
import type { Member } from "../types";
import { MembershipType } from "../types";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import { Card } from "../components/ui/card";
import { Alert, AlertDescription } from "../components/ui/alert";
import { Badge } from "../components/ui/badge";
import {
  Users,
  Plus,
  Mail,
  Phone,
  MapPin,
  Loader2,
  AlertCircle,
  CheckCircle,
} from "lucide-react";

export default function Members() {
  const [members, setMembers] = useState<Member[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [showAddForm, setShowAddForm] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<CreateMemberFormData>({
    resolver: zodResolver(createMemberSchema),
  });

  const loadMembers = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await membersApi.getAll();
      setMembers(data);
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("Failed to load members. Please try again.");
      }
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (data: CreateMemberFormData) => {
    setError(null);
    setSuccess(null);

    try {
      const newMember = await membersApi.create(data);
      setMembers((prev) => [newMember, ...prev]);
      reset();
      setSuccess(
        `Member registered successfully! Membership Number: ${newMember.membershipNumber}`
      );
      setShowAddForm(false);

      setTimeout(() => setSuccess(null), 5000);
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError("Failed to register member. Please try again.");
      }
    }
  };

  const getMembershipTypeColor = (type: MembershipType) => {
    switch (type) {
      case MembershipType.Student:
        return "bg-blue-100 text-blue-800";
      case MembershipType.Adult:
        return "bg-green-100 text-green-800";
      case MembershipType.Senior:
        return "bg-purple-100 text-purple-800";
      case MembershipType.Staff:
        return "bg-orange-100 text-orange-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  const getMembershipTypeLabel = (type: MembershipType) => {
    return MembershipType[type];
  };

  useEffect(() => {
    loadMembers();
  }, []);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Members</h1>
          <p className="text-muted-foreground">
            Manage library members and registrations
          </p>
        </div>
        <Button onClick={() => setShowAddForm(!showAddForm)}>
          <Plus className="mr-2 h-4 w-4" />
          Register Member
        </Button>
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

      {/* Add Member Form */}
      {showAddForm && (
        <Card className="p-6">
          <div className="flex items-center mb-6">
            <Plus className="h-5 w-5 mr-2" />
            <h2 className="text-lg font-semibold">Register New Member</h2>
          </div>

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
                onClick={() => setShowAddForm(false)}
              >
                Cancel
              </Button>
            </div>
          </form>
        </Card>
      )}

      {/* Members Display */}
      <div className="space-y-4">
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <Loader2 className="h-8 w-8 animate-spin mr-2" />
            <span>Loading members...</span>
          </div>
        ) : members.length === 0 ? (
          <Card className="p-8 text-center">
            <Users className="h-12 w-12 text-gray-400 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-gray-900 mb-2">
              No members registered yet
            </h3>
            <p className="text-gray-600 mb-4">
              Start by registering the first member to your library system.
            </p>
            <Button onClick={() => setShowAddForm(true)}>
              <Plus className="mr-2 h-4 w-4" />
              Register First Member
            </Button>
          </Card>
        ) : (
          <>
            <div className="flex items-center text-sm text-gray-600">
              <Users className="h-4 w-4 mr-1" />
              <span>
                {members.length} registered member
                {members.length !== 1 ? "s" : ""}
              </span>
            </div>

            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
              {members.map((member) => (
                <Card key={member.id} className="p-4">
                  <div className="flex justify-between items-start mb-3">
                    <div>
                      <h3 className="font-semibold">
                        {member.firstName} {member.lastName}
                      </h3>
                      <p className="text-sm text-gray-600">
                        {member.membershipNumber}
                      </p>
                    </div>
                    <Badge
                      className={getMembershipTypeColor(member.membershipType)}
                    >
                      {getMembershipTypeLabel(member.membershipType)}
                    </Badge>
                  </div>

                  <div className="space-y-2 text-sm text-gray-600">
                    <div className="flex items-center">
                      <Mail className="h-3 w-3 mr-2" />
                      <span className="truncate">{member.email}</span>
                    </div>
                    <div className="flex items-center">
                      <Phone className="h-3 w-3 mr-2" />
                      <span>{member.phone}</span>
                    </div>
                    <div className="flex items-center">
                      <MapPin className="h-3 w-3 mr-2" />
                      <span className="truncate">{member.address}</span>
                    </div>
                  </div>

                  <div className="mt-3 pt-3 border-t text-sm">
                    <span className="text-gray-600">Active loans: </span>
                    <span className="font-medium">
                      {member.activeBorrowingsCount}
                    </span>
                  </div>
                </Card>
              ))}
            </div>
          </>
        )}
      </div>
    </div>
  );
}
