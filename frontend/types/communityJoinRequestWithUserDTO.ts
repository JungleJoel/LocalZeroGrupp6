export interface CommunityJoinRequestWithUserDTO {
  id: string;
  userId: string;
  userFirstName: string;
  userLastName: string;
  communityId: string;
  isAccepted: boolean | null;
  createdAt: string;
}
