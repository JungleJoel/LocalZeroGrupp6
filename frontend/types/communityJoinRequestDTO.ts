export interface CommunityJoinRequestDTO {
  id: string;
  userId: string;
  communityId: string;
  isAccepted: boolean | null;
  createdAt: string;
}
