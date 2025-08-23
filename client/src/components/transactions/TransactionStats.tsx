import { Card } from "../ui/card";
import { BookOpen, Users, History } from "lucide-react";

interface TransactionStatsProps {
  availableBooks: number;
  activeMembers: number;
}

export default function TransactionStats({ availableBooks, activeMembers }: TransactionStatsProps) {
  return (
    <div className="grid gap-4 md:grid-cols-3">
      <Card className="p-4">
        <div className="flex items-center">
          <BookOpen className="h-8 w-8 text-blue-500 mr-3" />
          <div>
            <h3 className="font-medium">Available Books</h3>
            <p className="text-2xl font-bold text-blue-600">{availableBooks}</p>
          </div>
        </div>
      </Card>

      <Card className="p-4">
        <div className="flex items-center">
          <Users className="h-8 w-8 text-green-500 mr-3" />
          <div>
            <h3 className="font-medium">Active Members</h3>
            <p className="text-2xl font-bold text-green-600">{activeMembers}</p>
          </div>
        </div>
      </Card>

      <Card className="p-4">
        <div className="flex items-center">
          <History className="h-8 w-8 text-purple-500 mr-3" />
          <div>
            <h3 className="font-medium">Transactions</h3>
            <p className="text-2xl font-bold text-purple-600">
              Borrow & Return
            </p>
          </div>
        </div>
      </Card>
    </div>
  );
}