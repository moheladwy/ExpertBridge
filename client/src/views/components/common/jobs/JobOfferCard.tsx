import { JobOfferResponse } from "@/features/jobs/types";
import { Card, CardContent, CardHeader, CardTitle } from "../../ui/card";
import { Badge } from "../../ui/badge";
import { Calendar, CheckCircle, MapPin, Trash2, User, XCircle } from "lucide-react";
import { Button } from "../../ui/button";

const JobOfferCard = ({
  offer,
  type,
  onStatusUpdate,
  onDelete
}: {
  offer: JobOfferResponse;
  type: 'sent' | 'received';
  onStatusUpdate?: (id: string, status: 'accepted' | 'declined') => void;
  onDelete?: (id: string) => void;
}) => {
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(amount);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  return (
      <Card className="hover:shadow-md transition-shadow">
        <CardHeader className="pb-3">
          <div className="flex justify-between items-start">
            <CardTitle className="text-lg font-semibold">{offer.title}</CardTitle>
            <Badge variant="secondary" className="ml-2">
              {formatCurrency(offer.budget)}
            </Badge>
          </div>
          <div className="flex items-center gap-4 text-sm text-gray-600">
            <div className="flex items-center gap-1">
              <User className="h-4 w-4" />
              {offer.author.firstName} {offer.author.lastName}
            </div>
            <div className="flex items-center gap-1">
              <MapPin className="h-4 w-4" />
              {offer.area}
            </div>
            <div className="flex items-center gap-1">
              <Calendar className="h-4 w-4" />
              {formatDate(offer.createdAt)}
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <p className="text-gray-700 mb-4 line-clamp-3">{offer.description}</p>

          <div className="flex gap-2 flex-wrap">
            {type === 'received' && onStatusUpdate && (
              <>
                <Button
                  size="sm"
                  variant="default"
                  onClick={() => onStatusUpdate(offer.id, 'accepted')}
                  className="flex items-center gap-1"
                >
                  <CheckCircle className="h-4 w-4" />
                  Accept
                </Button>
                <Button
                  size="sm"
                  variant="outline"
                  onClick={() => onStatusUpdate(offer.id, 'declined')}
                  className="flex items-center gap-1"
                >
                  <XCircle className="h-4 w-4" />
                  Decline
                </Button>
              </>
            )}

            {type === 'sent' && onDelete && (
              <Button
                size="sm"
                variant="destructive"
                onClick={() => onDelete(offer.id)}
                className="flex items-center gap-1"
              >
                <Trash2 className="h-4 w-4" />
                Delete
              </Button>
            )}
          </div>
        </CardContent>
    </Card>
  );
};

export default JobOfferCard;
