import { Card } from "../ui/card";

export default function TransactionInfo() {
  return (
    <Card className="p-6">
      <h3 className="text-lg font-medium mb-4">Transaction Information</h3>
      <div className="grid gap-4 md:grid-cols-2">
        <div>
          <h4 className="font-medium text-gray-900 mb-2">
            Loan Periods by Membership Type:
          </h4>
          <ul className="text-sm text-gray-600 space-y-1">
            <li>• Student: 14 days (3 books max)</li>
            <li>• Adult: 21 days (5 books max)</li>
            <li>• Senior: 21 days (5 books max)</li>
            <li>• Staff: 30 days (10 books max)</li>
          </ul>
        </div>
        <div>
          <h4 className="font-medium text-gray-900 mb-2">Important Notes:</h4>
          <ul className="text-sm text-gray-600 space-y-1">
            <li>• Overdue fine: 2 MAD per day</li>
            <li>• Members with overdue books cannot borrow</li>
            <li>• Books must be available to borrow</li>
            <li>• Keep transaction ID for returns</li>
          </ul>
        </div>
      </div>
    </Card>
  );
}