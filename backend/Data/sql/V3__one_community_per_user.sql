-- Remove the composite primary key and replace with user_id as the sole primary key,
-- enforcing that each user can only be a resident of one community.
ALTER TABLE community_resident
    DROP CONSTRAINT community_resident_pkey;

ALTER TABLE community_resident
    ADD CONSTRAINT community_resident_pkey PRIMARY KEY (user_id);
