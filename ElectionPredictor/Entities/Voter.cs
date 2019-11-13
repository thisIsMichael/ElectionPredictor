using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElectionPredictor.Entities
{
    public class Voter
    {
        private const int NotSet = -1;

        [NoColumn]
        public AgeGroup? AgeGroupEnum;
        //{
        //    get => AgeGroup == NotSet ? (AgeGroup?)null : (AgeGroup)AgeGroup;
        //    set
        //    {
        //        if (value.HasValue)
        //        {
        //            AgeGroup = (int)value;
        //        }
        //        else
        //        {
        //            AgeGroup = NotSet;
        //        }
        //    }
        //}

        [NoColumn]
        public Gender? GenderEnum;
        //{
        //    get => Gender == NotSet ? (Gender?)null : (Gender)Gender;
        //    set
        //    {
        //        if (value.HasValue)
        //        {
        //            Gender = (int)value;
        //        }
        //        else
        //        {
        //            Gender = NotSet;
        //        }
        //    }
        //}

        [NoColumn]
        public Party? IntentionEnum;
        //{
        //    get => Intention == NotSet ? (Party?)null : (Party)Intention;
        //    set
        //    {
        //        if (value.HasValue)
        //        {
        //            Intention = (int)value;
        //        }
        //        else
        //        {
        //            Intention = NotSet;
        //        }
        //    }
        //}

        [NoColumn]
        public Party? PreviousVoteEnum;
        //{
        //    get => PreviousVote == NotSet ? (Party?)null : (Party)PreviousVote;
        //    set
        //    {
        //        if (value.HasValue)
        //        {
        //            PreviousVote = (int)value;
        //        }
        //        else
        //        {
        //            PreviousVote = NotSet;
        //        }
        //    }
        //}

        [NoColumn]
        public ReferendumResult? ReferendumResultEnum;
        //{
        //    get => ReferendumResult == NotSet ? (ReferendumResult?)null : (ReferendumResult)ReferendumResult;
        //    set
        //    {
        //        if (value.HasValue)
        //        {
        //            ReferendumResult = (int)value;
        //        }
        //        else
        //        {
        //            ReferendumResult = NotSet;
        //        }
        //    }
        //}

        [NoColumn]
        public Region RegionEnum;
        //{
        //    get => (Region)Region;
        //    set
        //    {
        //        Region = (int)value;
        //    }
        //}

        [NoColumn]
        public SocialGrade? SocialGradeEnum;
        //{
        //    get => SocialGrade == NotSet ? (SocialGrade?)null : (SocialGrade)SocialGrade;
        //    set
        //    {
        //        if (value.HasValue)
        //        {
        //            SocialGrade = (int)value;
        //        }
        //        else
        //        {
        //            SocialGrade = NotSet;
        //        }
        //    }
        //}

        public int AgeGroup => (int)AgeGroupEnum;
        public int Gender => (int)GenderEnum;
        public int Intention => (int)IntentionEnum;
        public int PreviousVote => (int)PreviousVoteEnum;
        public int ReferendumResult => (int)ReferendumResultEnum;
        public int Region => (int)RegionEnum;
        public int SocialGrade => (int)SocialGradeEnum;
    }
}
