import { useState, useEffect } from "react";
import { membersApi } from "../lib/api";
import type { Member } from "../types";
import AddMemberForm from "../components/members/AddMemberForm";
import MemberCard from "../components/members/MemberCard";
import { Button } from "../components/ui/button";
import { Alert, AlertDescription } from "../components/ui/alert";
import { Users, Plus, Loader2, AlertCircle, CheckCircle } from "lucide-react";
import { Card } from "../components/ui/card";

export default function Members() {
  const [members, setMembers] = useState<Member[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [showAddForm, setShowAddForm] = useState(false);

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

  const handleMemberAdded = (newMember: Member) => {
    setMembers((prev) => [newMember, ...prev]);
    setShowAddForm(false);
    setSuccess(
      `Member registered successfully! Membership Number: ${newMember.membershipNumber}`
    );
    setTimeout(() => setSuccess(null), 5000);
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
        <AddMemberForm
          onMemberAdded={handleMemberAdded}
          onCancel={() => setShowAddForm(false)}
        />
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
                <MemberCard key={member.id} member={member} />
              ))}
            </div>
          </>
        )}
      </div>
    </div>
  );
}
