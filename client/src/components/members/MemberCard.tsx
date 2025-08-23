import type { Member } from "../../types";
import { MembershipType } from "../../types";
import { Card } from "../ui/card";
import { Badge } from "../ui/badge";
import { Mail, Phone, MapPin } from "lucide-react";

interface MemberCardProps {
  member: Member;
}

export default function MemberCard({ member }: MemberCardProps) {
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

  return (
    <Card className="p-4">
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
  );
}