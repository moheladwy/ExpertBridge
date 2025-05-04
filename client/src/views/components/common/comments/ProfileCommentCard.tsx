// import React from "react";
// import { Link } from "react-router-dom";
// import { Comment } from "@/features/comments/types";
// import { ThumbUp, ThumbDown } from "@mui/icons-material";
// import TimeAgo from "../../custom/TimeAgo";
// import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";
//
// interface ProfileCommentCardProps {
//   comment: Comment;
//   postTitle?: string;
// }
//
// const ProfileCommentCard: React.FC<ProfileCommentCardProps> = ({ comment, postTitle }) => {
//   const netVotes = comment.upvotes - comment.downvotes;
//
//   return (
//     <div className="flex flex-col gap-3 p-3 border rounded-lg bg-white">
//       {postTitle && (
//         <div className="mb-2 text-sm text-gray-500">
//           <span className="font-semibold">On Post: </span>
//           <Link
//             to={`/feed/${comment.postId}`}
//             className="hover:text-blue-600 hover:underline"
//             dir="auto"
//           >
//             {postTitle}
//           </Link>
//         </div>
//       )}
//
//       {/* Comment Author */}
//       <div className="flex items-center space-x-3">
//         {comment.author?.profilePictureUrl ?
//           <img
//             src={comment.author.profilePictureUrl}
//             alt="Comment Author"
//             width={30}
//             height={30}
//             className="rounded-full"
//           />
//         :
//           <img
//             src={defaultProfile}
//             alt="Comment Author"
//             width={30}
//             height={30}
//             className="rounded-full"
//           />
//         }
//         <div>
//           {/* Name */}
//           <h4 className="text-sm font-semibold">
//             {comment.author.firstName + ' ' + comment.author.lastName}
//           </h4>
//           {/* Date of creation */}
//           <p className="text-xs text-gray-500">
//             <TimeAgo timestamp={comment.createdAt} />
//           </p>
//         </div>
//       </div>
//
//       {/* Comment Content */}
//       <div className="w-full break-words">
//         <p className="text-gray-700 whitespace-pre-wrap" dir="auto">{comment.content}</p>
//       </div>
//
//       {/* Vote Display */}
//       <div className="flex items-center space-x-3">
//         <div className="flex items-center text-gray-500">
//           <span className={`font-medium ${netVotes > 0 ? 'text-green-600' : netVotes < 0 ? 'text-red-600' : ''}`}>
//             {netVotes > 0 ? '+' : ''}{netVotes}
//           </span>
//           <span className="ml-1 text-xs text-gray-500">votes</span>
//         </div>
//
//         {/* View post link */}
//         <Link to={`/feed/${comment.postId}`} className="text-xs text-blue-600 hover:underline ml-auto">
//           View Discussion
//         </Link>
//       </div>
//     </div>
//   );
// };
//
// export default ProfileCommentCard;

import React from "react";
import {Link} from "react-router-dom";
import {Comment} from "@/features/comments/types";
import TimeAgo from "../../custom/TimeAgo";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";

interface ProfileCommentCardProps {
    comment: Comment;
    postTitle?: string;
}

const ProfileCommentCard: React.FC<ProfileCommentCardProps> = ({ comment, postTitle }) => {
    const netVotes = comment.upvotes - comment.downvotes;

    return (
        <div className="flex flex-col gap-3 p-3 border rounded-lg bg-white dark:bg-gray-800 dark:border-gray-700">
            {postTitle && (
                <div className="mb-2 text-sm text-gray-500 dark:text-gray-400">
                    <span className="font-semibold">On Post: </span>
                    <Link
                        to={`/feed/${comment.postId}`}
                        className="hover:text-blue-600 dark:hover:text-blue-400 hover:underline"
                        dir="auto"
                    >
                        {postTitle}
                    </Link>
                </div>
            )}

            {/* Comment Author */}
            <div className="flex items-center space-x-3">
                {comment.author?.profilePictureUrl ?
                    <img
                        src={comment.author.profilePictureUrl}
                        alt="Comment Author"
                        width={30}
                        height={30}
                        className="rounded-full"
                    />
                    :
                    <img
                        src={defaultProfile}
                        alt="Comment Author"
                        width={30}
                        height={30}
                        className="rounded-full"
                    />
                }
                <div>
                    {/* Name */}
                    <h4 className="text-sm font-semibold dark:text-white">
                        {comment.author.firstName + ' ' + comment.author.lastName}
                    </h4>
                    {/* Date of creation */}
                    <p className="text-xs text-gray-500 dark:text-gray-400">
                        <TimeAgo timestamp={comment.createdAt}/>
                    </p>
                </div>
            </div>

            {/* Comment Content */}
            <div className="w-full break-words">
                <p className="text-gray-700 dark:text-gray-300 whitespace-pre-wrap" dir="auto">{comment.content}</p>
            </div>

            {/* Vote Display */}
            <div className="flex items-center space-x-3">
                <div className="flex items-center text-gray-500 dark:text-gray-400">
          <span className={`font-medium ${
              netVotes > 0
                  ? 'text-green-600 dark:text-green-400'
                  : netVotes < 0
                      ? 'text-red-600 dark:text-red-400'
                      : 'dark:text-gray-400'
          }`}>
            {netVotes > 0 ? '+' : ''}{netVotes}
          </span>
                    <span className="ml-1 text-xs text-gray-500 dark:text-gray-400">votes</span>
                </div>

                {/* View post link */}
                <Link to={`/feed/${comment.postId}`}
                      className="text-xs text-blue-600 dark:text-blue-400 hover:underline dark:hover:text-blue-300 ml-auto">
                    View Discussion
                </Link>
            </div>
        </div>
    );
};

export default ProfileCommentCard;